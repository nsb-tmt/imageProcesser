using CommunityToolkit.Maui.Core;
using System.Diagnostics.CodeAnalysis;

namespace MauiApp1
{
    public partial class MainPage : ContentPage
    {
        private ImageBuilder imageBuilder;
        public MainPage()
        {
            InitializeComponent();
        }

        //ページ内コンテンツの操作

        public async Task<FileResult?> PickAndShow(PickOptions options)
        {
            //画像のソースを開く
            try
            {
                var result = await FilePicker.Default.PickAsync(options);
                if (result != null)
                {
                    if (result.FileName.EndsWith("jpg", StringComparison.OrdinalIgnoreCase) ||
                        result.FileName.EndsWith("png", StringComparison.OrdinalIgnoreCase))
                    {
                        imageBuilder = new ImageBuilder(result);
                        _ = getToastAsync("画像の読み込みに成功しました:" + result.FullPath);
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                // The user canceled or something went wrong
            }
            _ = getToastAsync("画像の読み込みに失敗しました");
            return null;
        }

        private void onChangedColor(object sender, EventArgs e)
        {
            //画像の色返還

            if (imageBuilder == null)
            {
                _ = getToastAsync("最初はヌル");
                return;
            }

            if (sender.Equals(MonoON))
            {
                imageBuilder.setMONOCOLOR(true);
                _ = getToastAsync("モノクロをオン");
            }
            else if (sender.Equals(MonoOFF))
            {
                imageBuilder.setMONOCOLOR(false);
                _ = getToastAsync("モノクロをオフ");
            }
            else
            {
                _ = getToastAsync("想定していない挙動");
            }

            imageView.Source = imageBuilder.getImage();

        }
        private void onChangedRotate(object sender, EventArgs e)
        {

            if (sender.Equals(RightRotate))
            {
                imageBuilder.setRotate(1);
                _ = getToastAsync("右に90°回転");
            }
            else if (sender.Equals(LeftRotate))
            {
                imageBuilder.setRotate(-1);
                _ = getToastAsync("右に90°回転");
            }
            else
            {
                _ = getToastAsync("想定していない挙動");
            }

            imageView.Source = imageBuilder.getImage();

        }

        //画面遷移のメソッド
        private async void OnTransitionClickedProcessing(object sender, EventArgs e)
        {
            //ページ１→ページ２、画像編集画面への遷移
            //画像を開く
            var result = await PickAndShow(PickOptions.Default);

            if (result != null)
            {
                //デフォルト画像の表示
                imageView.Source = imageBuilder.getImage();
                //デフォルトの保存ファイル名の設定
                saveFileName.Text = imageBuilder.getSource().FileName.Substring(0, imageBuilder.getSource().FileName.Length - 4) + "-edit";
                saveFileExtension.Text = imageBuilder.getExtension();
                input.IsVisible = false;
                process.IsVisible = true;
                output.IsVisible = false;

            }
        }
        private void OnTransitionClickedOutput(object sender, EventArgs e)
        {
            //ページ２→ページ３、ファイル保存の確認ページへの遷移
            if (saveFileName.Text == "")
            {
                //保存ファイル名がないとき
                _ = getToastAsync("保存ファイル名を入力してください");
                return;
            }
            //編集後のプレビューを表示
            //暫定的にデフォルト画像を表示
            imagePreview.Source = imageBuilder.getImage();
            //保存パスの指定
            saveFilePath.Text = "保存先：" + imageBuilder.getSource().FullPath.Substring(0, imageBuilder.getSource().FullPath.Length - imageBuilder.getSource().FileName.Length) + saveFileName.Text + imageBuilder.getExtension();
            input.IsVisible = false;
            process.IsVisible = false;
            output.IsVisible = true;

        }
        private void OnTransitionClickedInput(object sender, EventArgs e)
        {
            //ページ３→ページ１、ファイル保存後に初期ページへの遷移
            //画像を保存し、最初の画面に戻る
            String savePath = imageBuilder.getSource().FullPath.Substring(0, imageBuilder.getSource().FullPath.Length - imageBuilder.getSource().FileName.Length) + saveFileName.Text + imageBuilder.getExtension();
            bool result = imageBuilder.saveFile(savePath);
            if (result)
            {
                _ = getToastAsync("画像の保存に成功しました:" + savePath);
                input.IsVisible = true;
                process.IsVisible = false;
                output.IsVisible = false;
            }
            else
            {
                _ = getToastAsync("画像の保存に失敗しました:" + savePath);
            }

        }
        private void OnTransitionClickedReEdit(object sender, EventArgs e)
        {
            //ページ３→ページ２画像編集ページへ
            //画像編集のページに戻る
            input.IsVisible = false;
            process.IsVisible = true;
            output.IsVisible = false;
        }

        //細かい機能
        public async Task getToastAsync(String message)
        {
            //トーストの表示
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            ToastDuration duration = ToastDuration.Short;
            double fontSize = 14;
            var toast = CommunityToolkit.Maui.Alerts.Toast.Make(message, duration, fontSize);
            await toast.Show(cancellationTokenSource.Token);

        }

    }

}
